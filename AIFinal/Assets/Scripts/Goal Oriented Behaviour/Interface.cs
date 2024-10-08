using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mostly taken from https://gamedevelopment.tutsplus.com/tutorials/goal-oriented-action-planning-for-a-smarter-ai--cms-20793 github project // Taken from Github Project From http://github.com/sploreg/goap
public interface Interface
{
    HashSet<KeyValuePair<string, object>> getWorldState();

    HashSet<KeyValuePair<string, object>> createGoalState();

    void planFailed(HashSet<KeyValuePair<string, object>> failed);

    void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<BaseAction> actions);

    void actionsFinished();

    void planAborted(BaseAction aborter);

    bool move(BaseAction nextAction);
}
